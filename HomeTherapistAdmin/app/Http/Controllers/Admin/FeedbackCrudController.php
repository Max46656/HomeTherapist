<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\FeedbackRequest;
use App\Models\User;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;
use Backpack\CRUD\app\Library\Widget;
use Illuminate\Support\Facades\DB;

/**
 * Class FeedbackCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class FeedbackCrudController extends CrudController
{
    use \Backpack\CRUD\app\Http\Controllers\Operations\ListOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\CreateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\UpdateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\DeleteOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\ShowOperation;

    /**
     * Configure the CrudPanel object. Apply settings to all operations.
     *
     * @return void
     */
    public function setup()
    {
        CRUD::setModel(\App\Models\Feedback::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/feedback');
        CRUD::setEntityNameStrings('feedback', 'feedback');

    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {

        $highRatingCount = DB::table('feedbacks')
            ->where('rating', '>', 4)
            ->groupBy('user_id')
            ->select('user_id', DB::raw('COUNT(*) as count'))
            ->havingRaw('AVG(rating) > 4')
            ->get();
        $lowRatingCount = DB::table('feedbacks')
            ->where('rating', '<', 2)
            ->groupBy('user_id')
            ->select('user_id', DB::raw('COUNT(*) as count'))
            ->havingRaw('AVG(rating) < 2')
            ->get();
        $totalUsers = User::count();
        $highRatingCount = $highRatingCount->count();
        $lowRatingCount = $lowRatingCount->count();
        $highRatingPercentage = number_format(($highRatingCount / $totalUsers) * 100, 2);
        $lowRatingPercentage = number_format(($lowRatingCount / $totalUsers) * 100, 2);

        Widget::add()
            ->to('before_content')
            ->type('div')
            ->class('row')
            ->content([
                Widget::make(
                    [
                        'type' => 'progress',
                        'class' => 'card bg-dark text-white',
                        'wrapper' => ['class' => 'col-sm-3 col-md-3'],
                        'value' => $highRatingCount,
                        'description' => '評價超過4分治療師',
                        'progressClass' => 'progress-bar bg-success',
                        'progress' => $highRatingPercentage,
                        'hint' => $highRatingPercentage . '%',
                    ]
                ),
                Widget::make(
                    [
                        'type' => 'progress',
                        'class' => 'card bg-dark text-white',
                        'wrapper' => ['class' => 'col-sm-3 col-md-3'],
                        'value' => $lowRatingCount,
                        'description' => '評價低於2分治療師',
                        'progressClass' => 'progress-bar bg-danger',
                        'progress' => $lowRatingPercentage,
                        'hint' => $lowRatingPercentage . '%',
                    ]
                ),
            ]);

        // CRUD::column('order_id');
        CRUD::column('user_id')
            ->type('relationship')
            ->attribute('user.username')
            ->label('User')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    $user = \App\Models\User::where('staff_id', $entry->user_id)->first();
                    if ($user) {
                        return backpack_url('user/' . $user->id . '/show');
                    }
                    return backpack_url('user/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                $user = \App\Models\User::where('staff_id', $entry->user_id)->first();
                return $user->username ?? '-';
            });

        CRUD::column('order_id')
            ->type('relationship')
            ->attribute('order.id')
            ->label('Order')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    // dump($column);

                    $order = \App\Models\Order::find($column['value']);
                    if ($order) {
                        return backpack_url('order/' . $order->id . '/show');
                    }
                    return backpack_url('order/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                return $entry->order->id ?? '-';
            });

        CRUD::column('customer_id');
        CRUD::column('comments');
        CRUD::column('rating');
        CRUD::column('created_at');
        CRUD::column('updated_at');

        /**
         * Columns can be defined using the fluent syntax or array syntax:
         * - CRUD::column('price')->type('number');
         * - CRUD::addColumn(['name' => 'price', 'type' => 'number']);
         */
    }

    /**
     * Define what happens when the Create operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-create
     * @return void
     */
    protected function setupCreateOperation()
    {
        CRUD::setValidation(FeedbackRequest::class);

        CRUD::field('order_id');
        CRUD::field('user_id');
        CRUD::field('customer_id');
        CRUD::field('comments');
        CRUD::field('rating');

        /**
         * Fields can be defined using the fluent syntax or array syntax:
         * - CRUD::field('price')->type('number');
         * - CRUD::addField(['name' => 'price', 'type' => 'number']));
         */
    }

    /**
     * Define what happens when the Update operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-update
     * @return void
     */
    protected function setupUpdateOperation()
    {
        $this->setupCreateOperation();
    }
}
