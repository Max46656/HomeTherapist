<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\TherapistOpenServicesRequest;
use App\Models\Service;
use App\Models\TherapistOpenServices;
use App\Models\User;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;
use Backpack\CRUD\app\Library\Widget;
use Illuminate\Support\Facades\DB;

/**
 * Class TherapistOpenServicesCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class TherapistOpenServicesCrudController extends CrudController
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
        CRUD::setModel(\App\Models\TherapistOpenServices::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/therapist-open-services');
        CRUD::setEntityNameStrings('therapist open services', 'therapist open services');
        $this->crud->denyAccess(['create', 'update', 'delete']);
        // $this->crud->addFilter([
        //     'type' => 'text',
        //     'name' => 'username',
        //     'label' => 'Username',
        // ], function ($value) {
        //     $this->crud->query = $this->crud->query->whereHas('user', function ($query) use ($value) {
        //         $query->where('username', 'like', '%' . $value . '%');
        //     });
        // });

        // $this->crud->addFilter([
        //     'type' => 'text',
        //     'name' => 'service_name',
        //     'label' => 'Service Name',
        // ], function ($value) {
        //     $this->crud->query = $this->crud->query->whereHas('service', function ($query) use ($value) {
        //         $query->where('name', 'like', '%' . $value . '%');
        //     });
        // });

    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {

        $serviceCounts = DB::table('therapist_open_services')
            ->select('service_id', DB::raw('COUNT(*) as count'))
            ->groupBy('service_id')
            ->get()
            ->pluck('count', 'service_id')
            ->toArray();

        $cards = [];

        foreach ($serviceCounts as $serviceId => $count) {
            $service = \App\Models\Service::find($serviceId);
            if ($service) {
                $serviceName = $service->name;
                $cards[] = Widget::make([
                    'type' => 'card',
                    'class' => 'card bg-dark text-white',
                    'wrapper' => ['class' => 'col-sm-3 col-md-3'],
                    'content' => [
                        'header' => $serviceName,
                        'body' => $count . ' 次服務',
                    ],
                ]);
            }
        }

        Widget::add()
            ->to('before_content')
            ->type('div')
            ->class('row')
            ->content($cards);

        CRUD::column('service_id')
            ->type('relationship')
            ->attribute('service.id')
            ->label('Service')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    // dump($entry);
                    $service = \App\Models\Service::find($entry->service_id);
                    if ($service != null) {
                        return backpack_url('service/' . $service->id . '/show');
                    }
                    return backpack_url('service/');
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                $service = \App\Models\Service::find($entry->service_id);
                return $service->name ?? '-';
            })->filterType('select')
            ->filterSelectOptions(Service::pluck('name', 'id')->toArray());

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
            })->filterType('select')
            ->filterSelectOptions(User::pluck('username', 'staff_id')->toArray());

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
        CRUD::setValidation(TherapistOpenServicesRequest::class);

        CRUD::field('service_id');
        CRUD::field('user_id');

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
