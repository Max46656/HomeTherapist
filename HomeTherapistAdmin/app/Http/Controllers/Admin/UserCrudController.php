<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\UserRequest;
use App\Models\User;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;
use Backpack\CRUD\app\Library\Widget;
use Illuminate\Support\Facades\DB;

/**
 * Class UserCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class UserCrudController extends CrudController
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
        CRUD::setModel(\App\Models\User::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/user');
        CRUD::setEntityNameStrings('user', 'users');
        $this->crud->enableResponsiveTable();
        $this->crud->enablePersistentTable();

    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        $userCount = \App\Models\User::count();
        // $userInTaipei = \App\Models\User::where();
        $userInTaipei = User::whereRaw("address regexp '臺北'")->get()->count();
        $userInNewTaipei = User::whereRaw("address regexp '新北'")->get()->count();
        $userInKeelung = User::whereRaw("address regexp '基隆'")->get()->count();
        $userInTaoyuan = User::whereRaw("address regexp '桃園'")->get()->count();
        $userInHsinchu = User::whereRaw("address regexp '新竹'")->get()->count();
        $userInMiaoli = User::whereRaw("address regexp '新竹'")->get()->count();

        Widget::add()->to('before_content')->type('div')->class('row')->content([

            Widget::make()
                ->type('progress')
                ->class('card border-0 text-white bg-primary')
                ->progressClass('progress-bar')
                ->value($userCount)
                ->description('在籍治療師')
                ->progress(100 * (int) $userCount / 1000)
                ->hint(1000 - $userCount . '位到達下一個里程碑。'),
            Widget::make(
                [
                    'type' => 'card',
                    'class' => 'card bg-dark text-white',
                    'wrapper' => ['class' => 'col-sm-3 col-md-3'],
                    'content' => [
                        'header' => '北北基治療師人數',
                        'body' => "基隆" . $userInKeelung . "台北" . $userInTaipei . "新北" . $userInNewTaipei,
                    ],
                ]
            ),
            Widget::make(
                [
                    'type' => 'card',
                    'class' => 'card bg-dark text-white',
                    'wrapper' => ['class' => 'col-sm-3 col-md-3'],
                    'content' => [
                        'header' => '桃竹苗治療師人數',
                        'body' => "桃園" . $userInTaoyuan . "新竹" . $userInHsinchu . "苗栗" . $userInMiaoli,
                    ],
                ]
            ),
        ]);

        CRUD::column('username')->type('text');
        CRUD::column('email');
        CRUD::column('staff_id');
        CRUD::column('address')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    $googleMapsLink = "https://www.google.com/maps/search/?api=1&query=" . urlencode($entry->address);
                    return $googleMapsLink;
                },
                'target' => '_blank',
            ]);
        CRUD::column('average_rating')
            ->label('平均評價')
            ->type('text')
            ->sortAsDecimal(true)
            ->value(function ($entry) {
                $averageRating = DB::table('feedbacks')
                    ->where('user_id', $entry->staff_id)
                    ->avg('rating');
                if ($averageRating == null) {
                    return "0.0";
                }
                return round($averageRating, 2);
            });

        CRUD::column('phone_number');
        CRUD::column('email_confirmed')->type('boolean');
        CRUD::column('certificate_number');
        CRUD::column('latitude');
        CRUD::column('longitude');
        CRUD::column('radius');
        CRUD::column('password_hash');
        CRUD::column('security_stamp');
        CRUD::column('phone_number_confirmed')->type('boolean');
        CRUD::column('two_factor_enabled')->type('boolean');
        CRUD::column('lockout_end');
        CRUD::column('lockout_enabled')->type('boolean');
        CRUD::column('access_failed_count');
        CRUD::column('created_at')->type('datetime');
        CRUD::column('updated_at')->type('datetime');

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
        CRUD::setValidation(UserRequest::class);

        CRUD::field('username');
        CRUD::field('normalized_username');
        CRUD::field('email');
        CRUD::field('normalized_email');
        CRUD::field('email_confirmed');
        CRUD::field('password_hash');
        CRUD::field('security_stamp');
        CRUD::field('concurrency_stamp');
        CRUD::field('phone_number');
        CRUD::field('phone_number_confirmed');
        CRUD::field('two_factor_enabled');
        CRUD::field('lockout_end');
        CRUD::field('lockout_enabled');
        CRUD::field('access_failed_count');
        CRUD::field('staff_id');
        CRUD::field('address');
        CRUD::field('latitude');
        CRUD::field('longitude');

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
