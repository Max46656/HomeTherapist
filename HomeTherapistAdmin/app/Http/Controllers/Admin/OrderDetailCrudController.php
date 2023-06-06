<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\OrderDetailRequest;
use App\Models\Order;
use App\Models\OrderDetail;
use App\Models\Service;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;
use Backpack\CRUD\app\Library\Widget;

/**
 * Class OrderDetailCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class OrderDetailCrudController extends CrudController
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
        CRUD::setModel(\App\Models\OrderDetail::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/order-detail');
        CRUD::setEntityNameStrings('order detail', 'order details');
        $this->crud->denyAccess(['create', 'delete']);
    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        CRUD::addColumn([
            'name' => 'order_id',
            'label' => 'Order',
            'type' => 'relationship',
            'attribute' => 'order_id',
            'entity' => 'order',
            'model' => "App\Models\Order",
            'column' => 'id',
            'wrapper' => [
                'href' => function ($crud, $column, $entry, $related_key) {
                    $order = \App\Models\Order::find($column['value']);
                    if ($order) {
                        return backpack_url('order/' . $order->id . '/show');
                    }

                    return backpack_url('order/' . $related_key);
                },
                'target' => '_blank',
            ],
            'value' => function ($entry) {
                return $entry->order_id ?? '-';
            },
        ]);

        CRUD::addColumn([
            'name' => 'service_id',
            'label' => 'Service',
            'type' => 'relationship',
            'attribute' => 'service_id',
            'entity' => 'service',
            'model' => "App\Models\Service",
            'column' => 'id',
            'wrapper' => [
                'href' => function ($crud, $column, $entry, $related_key) {
                    $service = \App\Models\Service::find($entry->service_id);
                    if ($service) {
                        return backpack_url('service/' . $entry->service_id . '/show');
                    }

                    return backpack_url('service/');
                },
                'target' => '_blank',
            ],
            'value' => function ($entry) {
                return $entry->service->name ?? '-';
            },
        ]);
        CRUD::column('price');
        CRUD::column('note');
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
        CRUD::setValidation(OrderDetailRequest::class);

        CRUD::field('order_id');
        CRUD::field('service_id');
        CRUD::field('price');
        CRUD::field('note');

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